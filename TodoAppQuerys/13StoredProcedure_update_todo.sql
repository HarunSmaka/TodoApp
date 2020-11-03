create procedure proc_update_todo
(
	@newContent nvarchar(200),
	@currentContent nvarchar(200),
	@userID int
)
as
begin
	update Todos
	set Content = @newContent
	where Content = @currentContent and UserID = @userID
end