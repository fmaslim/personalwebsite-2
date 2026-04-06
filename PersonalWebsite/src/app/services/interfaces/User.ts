export interface User {
    id: number;
    firstName: string;
    lastName: string;
    maidenName?: string;
    age: number;
    gender: string;
    email: string;
    phone: string;
    username: string;
    birthDate: string;
    image: string;
    bloodGroup?: string;
    height?: string;
    weight?: string;
    eyeColor: string;
}

export interface UserResponse {
    users: User[];
    total: number;
    skip: number;
    limit: number;
}