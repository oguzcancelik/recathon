import Cookies from "js-cookie";
import {LogoutAction} from "../../infrastructure/constants/action-constants";
import {AuthToken} from "../../infrastructure/constants/application-constants";

const initState = {
    isLoggedIn: false
};

const authReducer = (state = initState, action) => {

    state = {
        ...state,
        isLoggedIn: !!Cookies.get(AuthToken)
    };

    switch (action.type) {
        case LogoutAction: {
            return {
                ...state,
                isLoggedIn: false
            };
        }
        default:
            return state;
    }
};

export default authReducer;
