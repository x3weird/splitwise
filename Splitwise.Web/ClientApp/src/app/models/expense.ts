
export interface Expense {
  id: string;
  description: string;
  amount: number;
  expenseType: string;
  note: string;
  createdOn: Date;
  addedBy: string;
  isDeleted: boolean;
}
