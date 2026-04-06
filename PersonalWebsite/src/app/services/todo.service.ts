import { Injectable } from '@angular/core';
import { Todo } from './interfaces/todo';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class TodoService {
    private apiUrl = 'https://jsonplaceholder.typicode.com/todos/1';
    private apiUrl_Many = 'https://jsonplaceholder.typicode.com/todos';
    constructor(private http: HttpClient) { }

    getTodo()
    {
        return this.http.get<Todo>(this.apiUrl);
    }

    getTodo_Many()
    {
        return this.http.get<Todo[]>(this.apiUrl_Many);
    }
}
