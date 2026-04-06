import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse } from '../services/interfaces/Product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
    productApiUrl = 'https://dummyjson.com/products';

    constructor(private httpClient: HttpClient) { }

    getProducts() {
        return [
            {
                name: 'Laptop - from service',
                price: 1200,
                category: 'Electronics'
            },
            {
                name: 'PhoneLaptop - from service',
                price: 800,
                category: 'Electronics'
            },
            {
                name: 'Desk ChairLaptop - from service',
                price: 250,
                category: 'Furniture'
            },
            {
                name: 'MonitorLaptop - from service',
                price: 300,
                category: 'Electronics'
            }
        ];
    }

    getProductsHttp(): Observable<ProductResponse> {
        return this.httpClient.get<ProductResponse>(this.productApiUrl);
    }
}
