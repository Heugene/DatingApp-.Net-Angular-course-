import { inject, Service } from '@angular/core';
import { AccountService } from './account-service';
import { tap } from 'rxjs';

@Service()
export class InitService {
    private accountService = inject(AccountService);

    init(){
        return this.accountService.refreshToken().pipe(
            tap(user => {
                if (user) {
                    this.accountService.setCurrentUser(user);
                    this.accountService.startTokenRefreshInterval();
                }
            })
        )
    }
}
