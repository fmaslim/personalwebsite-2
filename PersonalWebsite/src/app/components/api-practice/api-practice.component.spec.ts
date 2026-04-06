import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiPracticeComponent } from './api-practice.component';

describe('ApiPracticeComponent', () => {
  let component: ApiPracticeComponent;
  let fixture: ComponentFixture<ApiPracticeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ApiPracticeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApiPracticeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
