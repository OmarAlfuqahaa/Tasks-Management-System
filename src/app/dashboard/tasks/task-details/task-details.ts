import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {TasksService, UpdateTaskPayload} from '../../../core/tasks.service';
import {FormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import {MatOptionModule} from '@angular/material/core';
import {CommonModule} from '@angular/common';
import {MatIconModule} from '@angular/material/icon';
import {MatTab, MatTabGroup} from '@angular/material/tabs';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.html',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    FormsModule,
    CommonModule,
    MatIconModule,
    MatTab,
    MatTabGroup
  ],
  styleUrls: ['./task-details.css']
})
export class TaskDetailsComponent implements OnInit {
  task: any = null;
  newComment: string = '';
  fileToUpload: File | null = null;
  public statuses = [
    {value: 'todo', label: 'To Do'},
    {value: 'in-progress', label: 'In Progress'},
    {value: 'done', label: 'Done'}
  ];


  constructor(private route: ActivatedRoute, private taskService: TasksService) {
  }

  ngOnInit(): void {
    const id = +this.route.snapshot.paramMap.get('id')!;
    this.loadTask(id);
  }

  loadTask(id: number) {
    this.taskService.getTaskById(id).subscribe({
      next: data => {
        this.task = data;
        this.loadComments(id);
      },
      error: err => console.error(err)
    });
  }


  addComment() {
    if (!this.task) return;
    this.taskService.addComment(this.task.id, this.newComment).subscribe(() => {
      this.newComment = '';
      this.loadTask(this.task.id);
    });
  }

  loadComments(taskId: number) {
    this.taskService.getComments(taskId).subscribe(comments => {
      if (this.task) this.task.comments = comments;
    });
  }

  changeStatus(newStatus: string) {
    if (!this.task) return;

    const allowedStatuses = this.getAllowedStatuses();
    if (!allowedStatuses.includes(newStatus)) {
      console.warn('Cannot move status backwards.');
      return;
    }

    const payload: UpdateTaskPayload = {
      title: this.task.title,
      description: this.task.description ?? '',
      assignedUserId: this.task.assignedUserId,
      status: newStatus as 'todo' | 'in-progress' | 'done',
      attachments: this.task.attachments ?? []
    };


    if (!this.task) {
      console.error("Task is null - cannot update status");
      return;
    }

    this.taskService.updateTask(this.task.id, payload).subscribe({
      next: (updatedTask) => {
        if (!updatedTask) return;
        this.task = updatedTask;
        console.log(`Task updated: ${this.task.status}`);
      },
      error: err => console.error(err)
    });


  }


  onFileSelected(event: any) {
    this.fileToUpload = event.target.files[0];
  }

  uploadFile(fileInput: HTMLInputElement) {
    if (!this.fileToUpload) return;

    this.taskService.uploadAttachment(this.task.id, this.fileToUpload).subscribe((newAttachment: any) => {
      if (this.task) {
        if (!this.task.attachments) this.task.attachments = [];
        this.task.attachments.push(newAttachment);
      }
      fileInput.value = '';
      this.fileToUpload = null;
    });
  }


  public getAllowedStatuses(): string[] {
    if (!this.task) return [];
    const order = this.statuses.map(s => s.value); // ['todo','in-progress','done']
    const currentIndex = order.indexOf(this.task.status);
    const allowed: string[] = [this.task.status];
    if (currentIndex < order.length - 1) allowed.push(order[currentIndex + 1]);
    return allowed;
  }

  deleteAttachment(id: number) {
    this.taskService.deleteAttachment(this.task.id, id).subscribe(() => {
      this.loadTask(this.task.id);
    });
  }


}
