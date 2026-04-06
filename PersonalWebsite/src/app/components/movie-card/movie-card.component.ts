import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-movie-card',
  templateUrl: './movie-card.component.html',
  styleUrl: './movie-card.component.css'
})
export class MovieCardComponent {
    @Input() movie: any;
    @Output() movieSelected = new EventEmitter<any>();

    selectMovie() {
        this.movieSelected.emit(this.movie);
    }
}
