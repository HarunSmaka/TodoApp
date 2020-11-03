create procedure proc_insert_done_todo
(
	@userID int,
	@content nvarchar(200)
)
as
begin
	insert into DoneTodos(UserID,Content) 
	values(@userID,@content)
end