import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse2 } from './interfaces/Product';

@Injectable({
  providedIn: 'root'
})
export class Product2Service {
  private aprUrl = 'https://dummyjson.com/products';
  constructor(private httpClient: HttpClient) { }

    getProducts(): Observable<ProductResponse2> {
        return this.httpClient.get<ProductResponse2>(this.aprUrl);
    }
}
