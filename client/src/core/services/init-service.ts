import { inject, Service } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';
import { LikesService } from './likes-service';

@Service()
export class InitService {
    private accountService = inject(AccountService);
    private likesService = inject(LikesService)

    init(){
        const userString = localStorage.getItem('user');
        if (!userString) return of(null);
        const User = JSON.parse(userString);
        this.accountService.currentuser.set(User);
        this.likesService.getLikeIds();

        return of(null);
    }
}
