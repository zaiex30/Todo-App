import { Pipe, PipeTransform } from "@angular/core";
import { TodoItemDto } from "../web-api-client";

@Pipe ({name: 'checkIfUndefined'})
export class CheckIfUndefinedPipe implements PipeTransform {
  transform(value: TodoItemDto) : TodoItemDto | boolean {
    if(value === undefined)
    {
      return new TodoItemDto;
    }
    else {
      return value.tags.length <= 0;
    }
  }
}
