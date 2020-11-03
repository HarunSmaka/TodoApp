create procedure proc_delete_done_todo
(
	@content nvarchar(200),
	@userID int
)
as
begin
	delete DoneTodos
	where Content = @content and UserID = @userID
end