import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-employee-card',
  templateUrl: './app-employee-card.component.html',
  styleUrl: './app-employee-card.component.css'
})
export class AppEmployeeCardComponent {
    @Input() employee: any;

    @Output() employeeSelected_child = new EventEmitter<any>();

    selectEmployee_child() {
        this.employeeSelected_child.emit(this.employee);
    }
}
