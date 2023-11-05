import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add-edit-stu',
  templateUrl: './add-edit-stu.component.html',
  styleUrls: ['./add-edit-stu.component.css']
})
export class AddEditStuComponent implements OnInit {

  @Input() student:any;
  studentid:string = "";
  fullname: string ="";
  class: string ="";

  constructor(private service: SharedService) { }

  ngOnInit(): void {
    this.studentid = this.student.studentid;
    this.fullname = this.student.fullname;
    this.class = this.student.class;
  }

  addStudent(){
    var val = {studentid:this.studentid,
      fullname:this.fullname,
      class:this.class};
      this.service.addStudent(val).subscribe(res =>{
        alert(res.toString());
      })
  }

  updateStudent(){
    var val = {studentid:this.studentid,
      fullname:this.fullname,
      class:this.class};
      this.service.updateStudent(val).subscribe(res =>{
        alert(res.toString());
      })
  }



}
