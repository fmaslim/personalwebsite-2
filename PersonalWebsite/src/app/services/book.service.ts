import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BookService {

    constructor() { }

    getBooks() {
        return [
            {
                title: 'Atomic Habits - from service',
                author: 'James Clear',
                genre: 'Self-help',
            },
            {
                title: 'Deep Work - from service',
                author: 'Cal Newport',
                genre: 'Productivity',
            },
            {
                title: 'Clean Code - from service',
                author: 'Robert C Martin',
                genre: 'Programming',
            },
            {
                title: 'The Pragmatic Programmer - from service',
                author: 'Andrew Hunt',
                genre: 'Programming',
            },
        ];
    }
}
