import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

    constructor() { }

    getMovies() {
        return [
            {
                title: 'Inception',
                director: 'Christopher Nolan',
                genre: 'Sci-Fi'
            },
            {
                title: 'Interstellar',
                director: 'Franky Lin',
                genre: 'Sci-Fi'
            },
            {
                title: 'The Dark Knight',
                director: 'Sarah Lee',
                genre: 'Action'
            },
            {
                title: 'Coco',
                director: 'Pixar',
                genre: 'Animation'
            }
        ];
    }
}
