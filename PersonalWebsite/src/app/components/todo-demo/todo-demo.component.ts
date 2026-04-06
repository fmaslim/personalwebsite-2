import { Component, OnInit } from '@angular/core';
import { Todo } from '../../services/interfaces/todo';
import { TodoService } from '../../services/todo.service';

@Component({
  selector: 'app-todo-demo',
  templateUrl: './todo-demo.component.html',
  styleUrl: './todo-demo.component.css'
})
export class TodoDemoComponent implements OnInit {
    todo: Todo | null = null;
    todo_many: Todo[] = [];
    loading = true;
    error = '';
    debugError = '';

    constructor(private todoService: TodoService) { }

    ngOnInit() {
        this.loading = true;
        this.error = '';

        this.todoService.getTodo_Many().subscribe({
            next: (data) => {
                setTimeout(() => {
                    this.todo_many = data.slice(0, 50);
                    this.loading = false;
                    console.log(this.todo_many);
                }, 2000);
            },
            error: (err) => {
                console.error(err);
                this.loading = false;
                this.error = 'Failed to load todos.';
                this.debugError = JSON.stringify(err);
                this.debugError = `Status: ${err.status} | URL: ${err.url} | Message: ${err.message}`
            }
        });
    }
}
