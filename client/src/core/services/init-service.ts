import { inject, Service } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';

@Service()
export class InitService {
    accountService = inject(AccountService);

    init(){
        const userString = localStorage.getItem('user');
        if (!userString) return of(null);
        const User = JSON.parse(userString);
        this.accountService.currentuser.set(User);

        return of(null);
    }
}
