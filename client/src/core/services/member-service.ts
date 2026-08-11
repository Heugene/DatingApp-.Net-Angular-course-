import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../../types/member';

@Service()
export class MemberService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getMembers() {
        return this.http.get<Member[]>(this.baseUrl + 'members');

    }

    getMember(id: string) {
        return this.http.get<Member>(this.baseUrl + 'members/' + id);
    }
}
