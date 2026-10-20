import { Component, inject, OnInit, signal } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Photo } from '../../../types/photo';
import { AdminService } from '../../../core/services/admin-service';
import { MemberNamePipe } from '../../../core/pipes/member-name-pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-photo-management',
  imports: [MemberNamePipe, AsyncPipe, RouterLink],
  templateUrl: './photo-management.html',
  styleUrl: './photo-management.css',
})
export class PhotoManagement implements OnInit {
  private adminService = inject(AdminService);
  protected photos = signal<Photo[]>([]);

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => {
        this.photos.set(photos);
      }
    });
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos => photos.filter(p => p.id !== photoId));
      }
    });
  }

  rejectPhoto(photoId: number)
  {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos => photos.filter(p => p.id !== photoId));
      }
    });
  }

  // this one didn't work because of ERROR RuntimeError: NG0600: Writing to signals is not allowed
  // while Angular renders the template (eg. interpolations) 

  // So I just used a custom pipe instead

  // getMemberName(memberId: string): string {
  //   var displayName = 'N/A';
  //   this.memberService.getMember(memberId).subscribe({
  //     next: member => {
  //       displayName = member.displayName;
  //       console.log(displayName);
  //     }
  //   });
  //   return displayName;
  // }
}
