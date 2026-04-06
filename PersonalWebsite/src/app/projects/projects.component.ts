import { Component } from '@angular/core';
import { MovieService } from '../services/movie.service';
import { BookService } from '../services/book.service';
import { ProductService } from '../services/product.service';
import { EmployeeService } from '../services/employee.service';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.css'
})
export class ProjectsComponent {
    pageTitle = 'Projects';
    pageSubTitle = 'A selection of work focused on systems, structure, and practical engineering';
    featuredProject = 'Contoso Public Health IT';
    projectName = 'Contoso Public Health IT';

    projects = [
        {
            title: 'Contoso Public Health IT',
            description: 'A simulated enterprise Azure environment built to practice real-world architecture, naming conventions, Dev/Prod structure, and deployment flow.',
            tags: ['Azure', '.NET', 'Bicep', 'SQL', 'CI/CD'],
            proof: 'Demonstrates cloud architecture thinking, structured environment design, and practical platform engineering fundamentals.',
            github: '#',
            caseStudy: '#',
            image: '/website-banner.png',
        }, 
        {
            title: 'Personal Website',
            description: 'A premium Angular portfolio site designed to present my work, personal brand, and technical growth in a clean, structured way.',
            tags: ['Angular', 'TypeScript', 'CSS'],
            proof: 'Demonstrates front-end structure, routing, reusable layouts, and visual polish.',
            github: '#',
            caseStudy: '#',
            image: '/website-banner.png',
        },
        {
            title: '.NET API Training Lab',
            description: 'A hands-on API practice project focused on DTOs, service layers, controllers, validation, and realistic backend design patterns.',
            tags: ['.NET', 'C#', 'Web API', 'Swagger', 'SQL'],
            proof: 'Demonstrates clean API structure, separation of concerns, and realistic backend development workflow.',
            github: '#',
            caseStudy: '',
            image: '/website-banner.png',
        }
    ];
    employees = [
        {
            name: 'Alice Johnson',
            role: 'Frontend developer',
            department: 'IT'
        },
        {
            name: 'Bob Smith',
            role: 'Backend developer',
            department: 'IT'
        },
        {
            name: 'Carol Davis',
            role: 'QA engineer',
            department: 'Quality'
        },
        {
            name: 'David Lee',
            role: 'Project Manager',
            department: 'Management'
        },
    ];
    products = [
        {
            name: 'Laptop',
            price: 1200,
            category: 'Electronics'
        },
        {
            name: 'Phone',
            price: 800,
            category: 'Electronics'
        },
        {
            name: 'Desk Chair',
            price: 250,
            category: 'Furniture'
        },
        {
            name: 'Monitor',
            price: 300,
            category: 'Electronics'
        }
    ];
    books = [
        {
            title: 'Atomic Habits',
            author: 'James Clear',
            genre: 'Self-help',
        },
        {
            title: 'Deep Work',
            author: 'Cal Newport',
            genre: 'Productivity',
        },
        {
            title: 'Clean Code',
            author: 'Robert C Martin',
            genre: 'Programming',
        },
        {
            title: 'The Pragmatic Programmer',
            author: 'Andrew Hunt',
            genre: 'Programming',
        },
    ];
    movies = [
        {
            title: 'Inception',
            director: 'Christopher Nolan',
            genre: 'Sci-Fi',
        },
        {
            title: 'Interstellar',
            director: 'Franky Lin',
            genre: 'Sci-Fi',
        },
        {
            title: 'The Dark Knight',
            director: 'Sarah Lee',
            genre: 'Action',
        },
        {
            title: 'Coco',
            director: 'Pixar',
            genre: 'Animation',
        },
    ];
    movies2 = [
        {
            title: 'Inception',
            genre: 'Sci-Fi',
            year: '2010'
        },
        {
            title: 'The Dark Knight',
            genre: 'Action',
            year: '2008'
        },
        {
            title: 'Interstellar',
            genre: 'Sci-Fi',
            year: '2014'
        },
    ];

    selectedProject = this.projects[0];

    bannerUrl = '/website-banner-2.png';
    bannerAlt = 'Cloud computing and workspace banner';    
    showFeatured = true;

    selectedMovie: any = null;
    selectedBook: any = null;
    selectedProduct: any = null;
    selectedEmployee: any = null;

    toggleFeatured() {
        this.showFeatured = !this.showFeatured;
    }

    setSelectedProject(project: any) {
        this.selectedProject = project;
    }

    onMovieSelected(movie: any) {
        this.selectedMovie = movie;
        console.log('Selected movie:', movie);
    }

    onBookSelected_parent(book: any) {
        this.selectedBook = book;
        console.log('Selected book:', book);
    }

    onProductSelected_parent(product: any) {
        this.selectedProduct = product;
        console.log('Selected product:', product);
    }

    onEmployeeSelected_parent(employee: any) {
        this.selectedEmployee = employee;
        console.log('Selected employee:', employee);
    }

    //movies_svc_arr: any[] = [];
    selectedMovie_svc_arr: any = null;
    constructor(
        private movieService: MovieService,
        private bookService: BookService,
        private productService: ProductService,
        private employeeService: EmployeeService)
    {
        this.movies = this.movieService.getMovies();
        this.books = this.bookService.getBooks();
        this.products = this.productService.getProducts();
        this.employees = this.employeeService.getEmployees();
    }
}
