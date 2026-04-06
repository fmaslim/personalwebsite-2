import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-book-card',
  templateUrl: './book-card.component.html',
  styleUrl: './book-card.component.css'
})
export class BookCardComponent {
    @Input() book: any;
    @Output() bookSelected_child = new EventEmitter<any>();

    selectBook_child() {
        this.bookSelected_child.emit(this.book);
    }
}


