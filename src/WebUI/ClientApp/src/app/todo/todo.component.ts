import { Component, TemplateRef, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import {
  TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, PriorityLevelDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand, TodoItemTagsClient, TodoTagsDto, TodoItemTagVm, AddTodoItemTagCommand
} from '../web-api-client';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  allTags: TodoItemTagVm;
  priorityLevels: PriorityLevelDto[];
  selectedList: TodoListDto;
  selectedItem: TodoItemDto;
  allItems: TodoListDto;
  tagCountList: Array<any> = new Array<any>();
  tagCount: any = {
    tagId: 0,
    tagName: "",
    count: 0
  }
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  editTodoItemTagModalRef: BsModalRef;
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: ['']
  });


  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private tagsClient: TodoItemTagsClient,
    private modalService: BsModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.tagsClient.get().subscribe(
      result => {
        this.allTags = result;
        console.log("Tags", this.allTags);
      }
    );
    this.listsClient.get().subscribe(
      result => {
        this.lists = result.lists;
        this.priorityLevels = result.priorityLevels;
        if (this.lists.length) {
          this.selectedList = this.lists[0];
          this.setSelectedList(this.selectedList);
        }
      },
      error => console.error(error)
    );
  }

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  setSelectedList(selected: TodoListDto){
    this.selectedList = selected;
    localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
    console.log("List has been set to: ", this.selectedList);
    this.allTags.tagList.forEach(item => {
      let tag = this.allTags.todoItemTagList.filter(x => x.tagId === item.id);
      if(tag.length > 2)
      {
        this.tagCountList = [];
        this.tagCount = {
          tagId: item.id,
          tagName: item.tagName,
          count: tag.length
        }
        this.tagCountList.push(this.tagCount);
      }
    });
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      items: []
    } as TodoListDto;

    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);

        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title
    };

    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;
    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        (this.selectedList.title = this.listOptionsEditor.title),
          this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
        localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
      },
      error => console.error(error)
    );
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    console.log("Selected item", this.selectedItem);
    this.itemDetailsFormGroup.patchValue(this.selectedItem);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
        this.stopDeleteCountDown();
    });
  }

  updateItemDetails(): void {
    const item = new UpdateTodoItemDetailCommand(this.itemDetailsFormGroup.value);
    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(
            i => i.id !== this.selectedItem.id
          );
          const listIndex = this.lists.findIndex(
            l => l.id === item.listId
          );
          this.selectedItem.listId = item.listId;
          this.lists[listIndex].items.push(this.selectedItem);
        }

        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
        this.itemDetailsModalRef.hide();
        this.itemDetailsFormGroup.reset();
      },
      error => console.error(error)
    );
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem = item;
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
            localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => {
          localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
          console.log('Update succeeded.')
        },
        error => console.error(error)
      );
    }

    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () => {
          this.selectedList.items = this.selectedList.items.filter(t => t.id !== item.id);
          localStorage.setItem("AllItems", JSON.stringify(this.selectedList));
        },
        error => console.error(error)
      );
    }
  }

  showTagOptionsModal(template: TemplateRef<any>, item: TodoItemDto){
    this.selectedItem = item;
    console.log("Add tag to: ", this.selectedItem);
    this.editTodoItemTagModalRef = this.modalService.show(template);
  }

  addTag(tagId: any){
    let selectedItemTagId: TodoItemTagVm = new TodoItemTagVm;

    if(this.selectedItem.tags !== undefined){
      selectedItemTagId = this.selectedItem.tags.find(item => item.todoItemTag.tagId === tagId);
    }

    if(selectedItemTagId === undefined)
    {
      console.log("Tag is not yet added", tagId);
      let command =  {
        tagId: tagId,
        todoItemTagId: this.selectedItem.id
      } as AddTodoItemTagCommand
      this.tagsClient.addItemTag(command).subscribe(item => {
        this.selectedItem.tags.push(item);
      });
      console.log("Tag has been added.");
      this.editTodoItemTagModalRef.hide();
    }
    else{
      window.alert("Tag is already added.");
      console.log("Already added");
    }
  }

  removeTag(todoItemTagId: number, tagId: number){
    this.tagsClient.delete(todoItemTagId).subscribe(() => {
      if(this.selectedItem.tags.length === 1)
      {
        this.selectedItem.tags.shift();
      }
      else{
        this.selectedItem.tags = this.selectedItem.tags.filter(i => i.todoItemTag.tagId !== tagId);
      }
    });
    this.editTodoItemTagModalRef.hide();
  }

  stopDeleteCountDown() {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }

  filterItemsBy(tagId: number) {
    this.selectedList.items = JSON.parse(localStorage.getItem("AllItems")).items;
    console.log("Current list of TodoItems is: ", this.selectedList.items);
    if(tagId !== -1)
    {
      let filteredItems = [];

      this.selectedList.items.forEach(item => {
        let todoItemTagIds = item.tags.filter(x => x.todoItemTag.tagId === tagId).map(id => id.todoItemTag.todoItemTagId);

        todoItemTagIds.forEach(id => {
          let item = this.selectedList.items.filter(x => x.id === id);
          if (item !== null) {
            filteredItems.push(item);
          }
        });
      });

      this.selectedList.items = [];

      for (let i = 0; i < filteredItems.length; i++) {
        let item = filteredItems[i][0] as TodoItemDto;
        this.selectedList.items.push(item);
      }

      console.log("TodoItems filtered: ", this.selectedList.items);
    }
  }
}
