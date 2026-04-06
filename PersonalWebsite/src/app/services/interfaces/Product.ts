export interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    category: string;
    thumbnail: string;
}

export interface Product2 {
    id: number;
    title: string;
    description: string;
    category: string;
    price: number;
    discountPercentage: number;
    rating: number;
    stock: number;
    brand?: string;
    sku?: string;
    weight?: string;
    thumbnail?: string;
    images: string[];
}

export interface ProductResponse {
    products: Product[];
}

export interface ProductResponse2 {
    products: Product2[];
    total: number;
    skip: number;
    limit: number;
}