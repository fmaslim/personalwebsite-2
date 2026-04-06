import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsDemoComponent } from './products-demo.component';

describe('ProductsDemoComponent', () => {
  let component: ProductsDemoComponent;
  let fixture: ComponentFixture<ProductsDemoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProductsDemoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductsDemoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
