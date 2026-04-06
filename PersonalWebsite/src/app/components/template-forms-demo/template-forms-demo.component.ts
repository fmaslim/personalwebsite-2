import { Component } from '@angular/core';

@Component({
  selector: 'app-template-forms-demo',
  templateUrl: './template-forms-demo.component.html',
  styleUrl: './template-forms-demo.component.css'
})
export class TemplateFormsDemoComponent {
    user = {
        fullName: '',
        email: '',
        age: null,
    };

    onSubmit()
    {
        console.log('Form submitted:', this.user);
    }
}
