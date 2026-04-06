import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { ProjectsComponent } from './projects/projects.component';
import { ContactComponent } from './contact/contact.component';
import { ApiPracticeComponent } from './components/api-practice/api-practice.component';
import { TemplateFormsDemoComponent } from './components/template-forms-demo/template-forms-demo.component';
// import { TodoHttpPracticeComponent } from './components/todo-http-practice/todo-http-practice.component';

const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'about', component: AboutComponent },
    { path: 'projects', component: ProjectsComponent },
    { path: 'contact', component: ContactComponent },
    { path: 'api', component: ApiPracticeComponent },
    { path: 'template-forms-demo', component: TemplateFormsDemoComponent },
    // { path: 'todo-http', component: TodoHttpPracticeComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
