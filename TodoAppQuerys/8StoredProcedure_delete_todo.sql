create procedure proc_delete_todo
(
	@content nvarchar(200),
	@userID int
)
as
begin
	delete Todos
	where Content = @content and UserID = @userID
end