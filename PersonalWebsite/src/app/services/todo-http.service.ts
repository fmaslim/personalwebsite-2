import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TodoHttpResponse } from './interfaces/TodoHttp';

@Injectable({
  providedIn: 'root'
})
export class TodoHttpService {
    private apiUrl = 'https://dummyjson.com/todos';

    constructor(private httpService: HttpClient) { }

    getTodoHttp(): Observable<TodoHttpResponse> {
        return this.httpService.get<TodoHttpResponse>(this.apiUrl);
    }
}
