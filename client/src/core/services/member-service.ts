import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../../types/member';
import { Photo } from '../../types/photo';
import { EditableMember } from '../../types/editableMember';
import { tap } from 'rxjs';
import { PaginatedResult } from '../../types/pagination';
import { MemberParams } from '../../types/memberParams';

@Service()
export class MemberService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;
    editMode = signal(false);
    member = signal<Member | null>(null);

    getMembers(memberParams: MemberParams) {
        let params = new HttpParams();
        params = params.append('pageNumber', memberParams.pageNumber);
        params = params.append('pageSize', memberParams.pageSize);
        params = params.append('minAge', memberParams.minAge);
        params = params.append('maxAge', memberParams.maxAge);
        params = params.append('orderBy', memberParams.orderBy);
        if (memberParams.gender) params = params.append('gender', memberParams.gender);
        return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'members', {params: params});

    }

    getMember(id: string) {
        return this.http.get<Member>(this.baseUrl + 'members/' + id).pipe(
            tap(member => {
                this.member.set(member)
            })
        );
    }

    getMemberPhotos(id: string) {
        return this.http.get<Photo[]>(this.baseUrl + 'members/' + id + '/photos');
    }

    updateMember(member: EditableMember) {
        return this.http.put(this.baseUrl + 'members', member);
    }

    uploadPhoto(file: File) {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<Photo>(this.baseUrl + "members/add-photo", formData);
    }

    setMainPhoto(photo: Photo) {
        return this.http.put(this.baseUrl + "members/set-main-photo/" + photo.id, {});
    }

    deletePhoto(photoId: number) {
        return this.http.delete(this.baseUrl + "members/delete-photo/" + photoId);
    }
}
