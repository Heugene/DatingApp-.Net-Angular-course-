import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../../types/member';
import { Photo } from '../../types/photo';

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

    grtMemberPhotos(id: string) {
        return this.http.get<Photo[]>(this.baseUrl + 'members/' + id + '/photos');
    }
}
