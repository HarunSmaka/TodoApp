create procedure proc_insert_todo
(
	@userID int,
	@content nvarchar(200)
)
as
begin
	insert into Todos (UserID,Content) 
	values(@userID,@content)
end