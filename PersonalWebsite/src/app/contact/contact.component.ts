import { Component } from '@angular/core';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent {
    projects = [
        {
            title: 'Azure Architecture Lab',
            description: 'Hands-on cloud architecture practice in Azure'
        }, 
        {
            title: 'Personal Website',
            description: 'Angular portfolio site focused on systems and structure.'
        }
    ];
    contactMethods = [
        {
            title: 'Email',
            value: 'fmaslim@protonmail.com',
            link: 'mailto:fmaslim@protonmail.com'
        },
        {
            title: 'LinkedIn',
            value: 'https://www.linkedin.com/in/franky-lin-b3a2b9a/',
            link: 'https://www.linkedin.com/in/franky-lin-b3a2b9a/'
        },
        {
            title: 'YouTube',
            value: 'The Case Study Lab',
            link: 'https://www.youtube.com/channel/UCLDIxucjGGT6NnnxAHPO_6g'
        }
    ];
}
