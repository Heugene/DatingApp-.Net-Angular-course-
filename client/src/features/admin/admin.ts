import { NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { validate } from '@angular/forms/signals';
import { UserManagegment } from './user-managegment/user-managegment';
import { PhotoManagegment } from './photo-managegment/photo-managegment';

@Component({
  selector: 'app-admin',
  imports: [UserManagegment, PhotoManagegment],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin {
  protected accountService = inject(AccountService);
  activeTab = 'photos';
  tabs = [
    {label: 'Photo moderation', value: 'photos'},
    {label: 'User management', value: 'roles'},
  ];

  setTab(tab: string) {
    this.activeTab = tab;
  }
}
