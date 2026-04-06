import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Movie2CardComponent } from './movie2-card.component';

describe('Movie2CardComponent', () => {
  let component: Movie2CardComponent;
  let fixture: ComponentFixture<Movie2CardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Movie2CardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Movie2CardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
