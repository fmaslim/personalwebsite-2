import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, UserResponse } from './interfaces/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {    
    private apiUrl = 'https://dummyjson.com/users';

   constructor(private http: HttpClient) { }

    getUsers(): Observable<UserResponse> {
        return this.http.get<UserResponse>(this.apiUrl);
    }
}
