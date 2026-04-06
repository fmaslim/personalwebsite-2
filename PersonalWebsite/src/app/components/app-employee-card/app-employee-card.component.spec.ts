import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppEmployeeCardComponent } from './app-employee-card.component';

describe('AppEmployeeCardComponent', () => {
  let component: AppEmployeeCardComponent;
  let fixture: ComponentFixture<AppEmployeeCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppEmployeeCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppEmployeeCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
