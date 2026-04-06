import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

    constructor() { }

    getEmployees() {
        return [
            {
                name: 'Alice Johnson - from service',
                role: 'Frontend developer',
                department: 'IT'
            },
            {
                name: 'Bob Smith - from service',
                role: 'Backend developer',
                department: 'IT'
            },
            {
                name: 'Carol Davis - from service',
                role: 'QA engineer',
                department: 'Quality'
            },
            {
                name: 'David Lee - from service',
                role: 'Project Manager',
                department: 'Management'
            },
        ];
    }
}
