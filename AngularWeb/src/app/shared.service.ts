import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  readonly APIUrl = "http://localhost:32768/api";
  readonly APIUrlDept = "http://34.204.99.12:31479/api";
  constructor(private http: HttpClient) { }

  getStudentList(): Observable<any[]>{
    return this.http.get<any>(this.APIUrl + '/Student');
  }

  addStudent(val:any){
    return this.http.post(this.APIUrl + '/Student',val);
  }

  updateStudent(val:any){
    return this.http.put(this.APIUrl + '/Student', val);
  }

  deleteStudent(id: any){
    return this.http.delete(this.APIUrl + '/Student/'+id);
  }


  getDepartmentList(): Observable<any[]>{
    return this.http.get<any>(this.APIUrlDept + '/Department');
  }

  addDepartment(val:any){
    return this.http.post(this.APIUrlDept + '/Department', val);
  }

  updateDepartment(val:any){
    return this.http.put(this.APIUrlDept + '/Department', val);
  }

  deleteDepartment(id:any){
    return this.http.delete(this.APIUrlDept + '/Department/'+id);
  }

  
}
