import { inject, Service, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../../types/member';
import { PaginatedResult } from '../../types/pagination';

@Service()
export class LikesService {
    private baseUrl = environment.apiUrl;
    private http = inject(HttpClient);
    likeIds = signal<string[]>([]);

    toggleLike(targetMemberId: string) {
        return this.http.post(`${this.baseUrl}likes/${targetMemberId}`, {});
    }

    getLikes(predicate: string, pageNumber: number, pageSize: number ) {
        return this.http.get(this.baseUrl + `likes?pageNumber=${pageNumber}&pageSize=${pageSize}&predicate=${predicate}`); 
    }

    getLikeIds() {
        return this.http.get<string[]>(this.baseUrl + 'likes/list').subscribe({
            next: ids => this.likeIds.set(ids)
        })
    }

    clearIds() {
        this.likeIds.set([]);
    }
}
