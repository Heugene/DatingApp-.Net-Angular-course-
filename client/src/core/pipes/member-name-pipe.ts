import { inject, Pipe, PipeTransform } from '@angular/core';
import { MemberService } from '../services/member-service';
import { map, Observable } from 'rxjs';

@Pipe({
  name: 'memberName',
})
export class MemberNamePipe implements PipeTransform {
  private memberService = inject(MemberService);

  transform(memberId: string): Observable<string> {
    return this.memberService.getMember(memberId).pipe(map(member => member.displayName ?? 'N/A'));
  }
}
