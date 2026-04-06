import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent {
    @Input() product: any;

    @Output() productSelected_child = new EventEmitter<any>();

    selectProduct_child() {
        this.productSelected_child.emit(this.product);
    }
}
