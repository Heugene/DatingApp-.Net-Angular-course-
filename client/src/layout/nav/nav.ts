import { Component, inject, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService = inject(AccountService)
  protected creds: any = {}
  private router = inject(Router);

  login() {
    this.accountService.login(this.creds).subscribe({
      next: result =>{
        this.router.navigateByUrl('/members');
        this.creds = {}
      },
      error: error => alert(error.message)
      })
  }

  logout(){
    this.accountService.logout()
    this.router.navigateByUrl('/');
  }
}
