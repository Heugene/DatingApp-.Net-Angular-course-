import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { User } from '../../../types/user';

@Component({
  selector: 'app-user-managegment',
  imports: [],
  templateUrl: './user-managegment.html',
  styleUrl: './user-managegment.css',
})
export class UserManagegment implements OnInit {
  private adminService = inject(AdminService);
  protected users = signal<User[]>([]);

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: users => this.users.set(users)
    });
  }
}
