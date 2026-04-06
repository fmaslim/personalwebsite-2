import { Component,Input } from '@angular/core';

@Component({
  selector: 'app-movie2-card',
  templateUrl: './movie2-card.component.html',
  styleUrl: './movie2-card.component.css'
})
export class Movie2CardComponent {
    @Input() movie: any;
}
