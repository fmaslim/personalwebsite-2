import { Component, OnInit } from '@angular/core';
import { Product, ProductResponse } from '../../services/interfaces/Product';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-products-demo',
  templateUrl: './products-demo.component.html',
  styleUrl: './products-demo.component.css'
})
export class ProductsDemoComponent implements OnInit {
    products_many: Product[] = [];
    loading = true;
    error = '';
    debugError = '';
    constructor(private productService: ProductService) { }

    ngOnInit(): void {
        this.loading = true;
        this.error = '';

        this.productService.getProductsHttp().subscribe({
            next: (response: ProductResponse) => {
                this.products_many = response.products.slice(0, 50);
                this.loading = false;
                console.log(this.products_many);
            },
            error: (err) => {
                console.error(err);
                this.loading = false;
                this.error = 'Failed to load products.';
                this.debugError = JSON.stringify(err);
                this.debugError = `Status: ${err.status} | URL: ${err.url} | Message: ${err.message}`;
            }
        });
    }
}
