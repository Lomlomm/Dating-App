import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member | undefined;
  
  constructor(private membersService: MembersService, private toastr: ToastrService) {}
  
  ngOnInit(): void {
  }

  addLike(member: Member) {
    this.membersService.addLike(member.userName).subscribe({
      next: () => this.toastr.success("Te gusta " + member.knownAs)
    });
  }
}
