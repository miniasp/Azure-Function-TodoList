import { Component, OnInit } from '@angular/core';
import { TodoService } from './todo.service';
import { Todo } from './todo';
@Component({
  selector: 'app-todo',
  providers: [TodoService],
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  newTask: string;
  todos = [] as Todo[];
  constructor(private todoService: TodoService) { }

  ngOnInit() {
    this.todoService
      .getAll()
      .subscribe(todos => {
        this.todos = todos;
      });
  }

  add() {
    this.todoService
      .add(this.newTask)
      .subscribe(newTodo => {
        this.todos.push(newTodo);
      });
      this.newTask = null;
  }

  done(todo: Todo) {
    todo.Done = !todo.Done;
    this.todoService
      .done(todo.Key)
      .subscribe(rsp => {
        // ohohoh lazy boy
      });
  }

  remove(index: number) {
    const toRemoveTodo = this.todos[index];
    this.todos.splice(index, 1);

    this.todoService
      .remove(toRemoveTodo.Key)
      .subscribe(rsp => {
        // ohohoh lazy boy
      });
  }

}
