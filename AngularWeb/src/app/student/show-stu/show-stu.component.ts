import { Component, OnInit } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-show-stu',
  templateUrl: './show-stu.component.html',
  styleUrls: ['./show-stu.component.css']
})
export class ShowStuComponent implements OnInit {
  studentList:any = [];
  modalTitle:any;
  activateAddEditStuCom:boolean = false;
  student:any;
  
  constructor(private sharedService: SharedService) { }

  ngOnInit(): void {
    this.refreshStudentList();
  }

  refreshStudentList() {
    // debugger;
    this.sharedService.getStudentList().subscribe(data =>{
      this.studentList = data;
      // debugger;
    });  
  }

  AddStudent(){
    this.student={
      studentid:0,
      fullname:"",
      class:""
    }
    this.modalTitle = "Add Student";
    this.activateAddEditStuCom = true;
  }

  EditStudent(item: any){
    // debugger;
    this.student = item;
    this.activateAddEditStuCom = true;
    this.modalTitle = "Update Student";
  }

  deleteClick(item: any){
    if(confirm('Are you sure??')){
      this.sharedService.deleteStudent(item.studentid).subscribe(data =>{
        alert(data.toString());
        this.refreshStudentList();
      })
    }
  }

  closeClick(){
    this.activateAddEditStuCom=false;
    this.refreshStudentList();
  }



}
