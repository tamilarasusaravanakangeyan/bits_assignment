import { Component, OnInit,Input } from '@angular/core';
import { SharedService } from "src/app/shared.service";

@Component({
  selector: 'app-add-edit-dep',
  templateUrl: './add-edit-dep.component.html',
  styleUrls: ['./add-edit-dep.component.css']
})
export class AddEditDepComponent implements OnInit {
  @Input() department:any;
  departmentid:any=0;
  departmentname:any="";

  constructor(private service:SharedService) { }

  ngOnInit(): void {
    this.departmentid = this.department.departmentid;
    this.departmentname = this.department.departmentname;
  }

  addDepartment(){
    var val={
      departmentid:this.departmentid,
      departmentname:this.departmentname
    }

    this.service.addDepartment(val).subscribe(res => {
      alert(res.toString());
    })
  }

  updateDepartment(){
    var val={
      departmentid:this.departmentid,
      departmentname:this.departmentname
    }

    this.service.updateDepartment(val).subscribe(res => {
      alert(res.toString());
    })
  }

}
