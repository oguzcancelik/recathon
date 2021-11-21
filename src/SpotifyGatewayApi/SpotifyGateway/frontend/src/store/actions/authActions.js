import Cookies from "js-cookie";
import {LogoutAction} from "../../infrastructure/constants/action-constants";
import {AuthToken} from "../../infrastructure/constants/application-constants";

export const logout = () => {
    Cookies.remove(AuthToken);
    return {type: LogoutAction};
};
