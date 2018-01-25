import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { Todo } from './todo';

@Injectable()
export class TodoService {

  private baseUrl = 'http://localhost:7071/api/todo';
  constructor(private http: HttpClient) { }

  getAll(): Observable<Todo[]> {
    return this.http
      .get<Todo[]>(this.baseUrl);
  }

  get(id: string): Observable<Todo> {
    return this.http
      .get<Todo>(`${this.baseUrl}/${id}`);
  }

  add (task: string): Observable<Todo> {
    return this.http
      .post<Todo>(this.baseUrl, task);
  }

  done (id: string): Observable<any> {
    return this.http
      .put(`${this.baseUrl}/${id}`, null);
  }

  remove (id: string): Observable<any> {
    return this.http
      .delete(`${this.baseUrl}/${id}`);
  }
}
