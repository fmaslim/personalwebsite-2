export interface TodoHttp {
    id: number;
    todo: string;
    completed: boolean;
    userId: number;
}

export interface TodoHttpResponse {
    todos: TodoHttp[];
    total: number;
    skip: number;
    limit: number;
}