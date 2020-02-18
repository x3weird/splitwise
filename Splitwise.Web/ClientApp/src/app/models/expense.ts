
export interface Expense {
  Id: string;
  Description: string;
  Amount: number;
  ExpenseType: string;
  Note: string;
  CreatedOn: Date;
  AddedBy: string;
  IsDeleted: boolean;
}
